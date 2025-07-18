﻿using AiryPayNew.Domain.Entities.Withdrawals;
using AiryPayNew.Infrastructure.Data.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiryPayNew.Infrastructure.Data.EntityTypeConfigurations;

internal class WithdrawalEntityTypeConfiguration : IEntityTypeConfiguration<Withdrawal>
{
    public void Configure(EntityTypeBuilder<Withdrawal> builder)
    {
        builder.ToTable("withdrawals")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                id => new WithdrawalId(id))
            .HasValueGenerator<IdValueGenerator<WithdrawalId>>()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ReceivingAccountNumber)
            .HasMaxLength(128);
            
        builder.HasOne(x => x.Shop)
            .WithMany(x => x.Withdrawals);
    }
}